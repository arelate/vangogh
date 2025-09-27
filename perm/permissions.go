package perm

import "github.com/boggydigital/author"

const (
	RoleAdmin = "admin"
	RoleUser  = "user"
	RoleDemo  = "demo"
)

const (
	ReadUpdates author.Permission = iota
	ReadSearch
)

func GetRolesPermissions() map[string][]author.Permission {

	return map[string][]author.Permission{
		RoleAdmin: {ReadUpdates, ReadSearch},
		RoleUser:  {ReadUpdates},
		RoleDemo:  {},
	}
}
