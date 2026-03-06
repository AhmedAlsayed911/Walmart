SELECT * FROM Security.Users
SELECT * FROM Security.Roles

INSERT INTO security.UserRoles (UserId , RoleId) SELECT '6db86cf4-5c79-46e0-978b-8be0831682cc', Id FROM security.Roles

DELETE FROM [security].[UserRoles] WHERE UserId = '6db86cf4-5c79-46e0-978b-8be0831682cc'