package url_helpers

import (
	"net/url"
	"strings"
)

func Value(u *url.URL, arg string) string {
	if u == nil {
		return ""
	}

	q := u.Query()
	return q.Get(arg)
}

func Values(u *url.URL, arg string) []string {
	if Flag(u, arg) {
		val := Value(u, arg)
		return strings.Split(val, ",")
	}
	return nil
}

func Flag(u *url.URL, arg string) bool {
	if u == nil {
		return false
	}

	q := u.Query()
	return q.Has(arg)
}
