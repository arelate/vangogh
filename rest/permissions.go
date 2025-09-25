package rest

import "github.com/boggydigital/author"

const (
	RoleAdmin = "admin"
	RoleUser  = "user"
)

const (
	ReadUpdates author.Permission = iota
	ReadSearch
)

func GetRoles() map[string][]author.Permission {

	return map[string][]author.Permission{
		RoleAdmin: {ReadUpdates, ReadSearch},
		RoleUser:  {ReadUpdates},
	}
}
