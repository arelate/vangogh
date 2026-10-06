package perm

import "github.com/boggydigital/author"

const (
	RoleAdmin = "admin"
	RoleUser  = "user"
)

const (
	ReadUpdates author.Permission = iota
	ReadSearch
	ReadAccountProducts
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
	WriteCookies
	ReadAccessTokens
)

var (
	browsePermissions       = []author.Permission{ReadUpdates, ReadSearch, ReadProductData, ReadImages}
	ownedPermissions        = []author.Permission{ReadAccountProducts, ReadFiles, ReadApi}
	accountPermissions      = []author.Permission{ReadWishlist, WriteWishlist, ReadTagId, WriteTagId, ReadLocalTags, WriteLocalTags, WriteCookies}
	debugPermissions        = []author.Permission{ReadLogs, ReadDebug}
	accessTokensPermissions = []author.Permission{ReadAccessTokens}
)

func GetRolesPermissions() map[string][]author.Permission {

	rolesPermissions := make(map[string][]author.Permission)

	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], browsePermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], ownedPermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], accountPermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], debugPermissions...)
	rolesPermissions[RoleAdmin] = append(rolesPermissions[RoleAdmin], accessTokensPermissions...)

	rolesPermissions[RoleUser] = append(rolesPermissions[RoleUser], browsePermissions...)
	rolesPermissions[RoleUser] = append(rolesPermissions[RoleUser], ownedPermissions...)

	return rolesPermissions
}
