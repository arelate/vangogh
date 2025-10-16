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
	ReadOwned
	ReadProductData
	ReadImages
	ReadFiles
	ReadWishlist
	WriteWishlist
	ReadTagId
	WriteTagId
	ReadLocalTags
	WriteLocalTags
	ReadApi
	ReadDebug
	ReadLogs
)

var (
	browsePermissions  = []author.Permission{ReadUpdates, ReadSearch, ReadProductData, ReadImages}
	ownedPermissions   = []author.Permission{ReadOwned, ReadFiles, ReadApi}
	accountPermissions = []author.Permission{ReadWishlist, WriteWishlist, ReadTagId, WriteTagId, ReadLocalTags, WriteLocalTags}
	debugPermissions   = []author.Permission{ReadLogs, ReadDebug}
)

func GetRolesPermissions() map[string][]author.Permission {

	rolesPermissions := make(map[string][]author.Permission)

	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], browsePermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], ownedPermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], accountPermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], debugPermissions...)

	rolesPermissions[RoleUser] = append(rolesPermissions[RoleUser], browsePermissions...)
	rolesPermissions[RoleUser] = append(rolesPermissions[RoleUser], ownedPermissions...)

	rolesPermissions[RoleDemo] = append(rolesPermissions[RoleDemo], browsePermissions...)

	return rolesPermissions
}
