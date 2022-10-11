package rest

import (
	"github.com/arelate/vangogh_local_data"
	"net/url"
)

func sortDescFromUrl(u *url.URL) (string, bool) {
	q := u.Query()
	sort := q.Get(vangogh_local_data.SortProperty)
	if sort == "" {
		sort = vangogh_local_data.TitleProperty
	}
	desc := false
	if q.Get(vangogh_local_data.DescendingProperty) == "true" {
		desc = true
	}

	return sort, desc
}
