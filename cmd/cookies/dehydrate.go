package cookies

import "net/http"

func dehydrate(cookies []*http.Cookie) map[string]string {
	ckv := make(map[string]string, len(cookies))
	for _, c := range cookies {
		ckv[c.Name] = c.Value
	}
	return ckv
}
