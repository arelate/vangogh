package index

import "github.com/boggydigital/vangogh/internal/gog/media"

// Indexes are used for enumeration and sorting
type Index struct {
	MediaType media.Type
	Hash      string
	Created   int64
	Modified  int64
}
