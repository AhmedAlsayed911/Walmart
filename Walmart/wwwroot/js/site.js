document.addEventListener('DOMContentLoaded', () => {
	const forms = document.querySelectorAll('[data-live-search-form="true"][data-search-mode="client"]');

	forms.forEach((form) => {
		const input = form.querySelector('[data-live-search-input="true"]');
		const scopeSelector = form.dataset.filterScope;
		if (!input || !scopeSelector) {
			return;
		}

		const scope = document.querySelector(scopeSelector);
		if (!scope) {
			return;
		}

		const itemSelector = form.dataset.filterItemSelector || '[data-filter-item="true"]';
		const items = Array.from(scope.querySelectorAll(itemSelector));
		if (items.length === 0) {
			return;
		}

		const emptyState = form.dataset.filterEmptyState
			? document.querySelector(form.dataset.filterEmptyState)
			: null;

		const pagination = form.dataset.filterPagination
			? document.querySelector(form.dataset.filterPagination)
			: null;

		const applyFilter = () => {
			const searchTerm = input.value.trim().toLowerCase();
			let visibleCount = 0;

			items.forEach((item) => {
				const text = (item.dataset.searchText || item.textContent || '').toLowerCase();
				const isMatch = !searchTerm || text.includes(searchTerm);

				item.classList.toggle('d-none', !isMatch);
				if (isMatch) {
					visibleCount += 1;
				}
			});

			if (emptyState) {
				emptyState.classList.toggle('d-none', visibleCount > 0);
			}

			if (pagination) {
				pagination.classList.toggle('d-none', searchTerm.length > 0);
			}
		};

		input.addEventListener('input', applyFilter);

		form.addEventListener('submit', (event) => {
			event.preventDefault();
			applyFilter();
		});

		const clearButton = form.querySelector('[data-live-search-clear="true"]');
		if (clearButton) {
			clearButton.addEventListener('click', (event) => {
				event.preventDefault();
				input.value = '';
				applyFilter();
				input.focus();
			});
		}

		applyFilter();
	});
});

// Global Confirmation Modal helper
window.showConfirmModal = function(title, body, confirmCallback) {
	const modalEl = document.getElementById('globalConfirmModal');
	if (!modalEl) {
		if (confirm(body)) confirmCallback();
		return;
	}

	document.getElementById('globalConfirmTitle').textContent = title;
	document.getElementById('globalConfirmBody').textContent = body;

	const confirmBtn = document.getElementById('globalConfirmBtn');
	const newConfirmBtn = confirmBtn.cloneNode(true);
	confirmBtn.parentNode.replaceChild(newConfirmBtn, confirmBtn);

	const modal = new bootstrap.Modal(modalEl);

	newConfirmBtn.addEventListener('click', () => {
		modal.hide();
		confirmCallback();
	});

	modal.show();
};

// Form submission interceptor for standard delete confirm dialogs
document.addEventListener('submit', (event) => {
	const form = event.target;
	const onsubmitAttr = form.getAttribute('onsubmit') || '';
	
	if (onsubmitAttr.includes('confirm(')) {
		if (form.dataset.confirmed === 'true') {
			form.removeAttribute('data-confirmed');
			return true;
		}

		event.preventDefault();

		let message = "Are you sure you want to perform this action?";
		const match = onsubmitAttr.match(/confirm\(['"](.*)['"]\)/);
		if (match && match[1]) {
			message = match[1];
		}

		window.showConfirmModal("Confirm Action", message, () => {
			form.setAttribute('data-confirmed', 'true');
			form.submit();
		});

		return false;
	}
});

// Global Toast Notification manager
window.showToast = function(message, type = 'success') {
	const container = document.getElementById('toast-container');
	if (!container) return;

	const toastEl = document.createElement('div');
	toastEl.className = `toast align-items-center border-0 shadow-lg mb-2`;
	toastEl.role = 'alert';
	toastEl.ariaLive = 'assertive';
	toastEl.ariaAtomic = 'true';

	// Define style variables based on type
	let bgClass = 'bg-success text-white';
	let icon = '✅';
	if (type === 'danger' || type === 'error') {
		bgClass = 'bg-danger text-white';
		icon = '❌';
	} else if (type === 'warning') {
		bgClass = 'bg-warning text-dark';
		icon = '⚠️';
	} else if (type === 'info') {
		bgClass = 'bg-info text-white';
		icon = 'ℹ️';
	}

	toastEl.className += ` ${bgClass}`;

	toastEl.innerHTML = `
		<div class="d-flex">
			<div class="toast-body d-flex align-items-center gap-2 py-3 px-4" style="font-size: 0.95rem; font-weight: 600;">
				<span>${icon}</span>
				<div>${message}</div>
			</div>
			<button type="button" class="btn-close btn-close-white me-3 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
		</div>
	`;

	container.appendChild(toastEl);
	const toast = new bootstrap.Toast(toastEl, { delay: 4000 });
	toast.show();

	// Remove toast from DOM after hidden to save memory
	toastEl.addEventListener('hidden.bs.toast', () => {
		toastEl.remove();
	});
};
