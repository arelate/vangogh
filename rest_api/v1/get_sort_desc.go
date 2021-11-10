package v1

import (
	"net/url"
)

func getSortDesc(u *url.URL) (string, bool) {
	q := u.Query()
	sort := q.Get("sort")
	if sort == "" {
		sort = defaultSort
	}
	desc := false
	if q.Get("desc") == "true" {
		desc = true
	}

	return sort, desc
}
