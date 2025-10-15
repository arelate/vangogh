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

func GetRolesPermissions() map[string][]author.Permission {

	return map[string][]author.Permission{
		RoleAdmin: {ReadUpdates, ReadSearch, ReadProductData, ReadImages,
			ReadFiles, ReadLocalTags, WriteLocalTags, ReadApi,
			ReadWishlist, WriteWishlist, ReadTagId, WriteTagId, ReadDebug, ReadLogs},
		RoleUser: {ReadUpdates, ReadSearch, ReadProductData, ReadImages,
			ReadFiles, ReadLocalTags, WriteLocalTags, ReadApi},
		RoleDemo: {ReadUpdates, ReadSearch, ReadProductData, ReadImages},
	}

}
