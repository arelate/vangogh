package v1

import (
	"fmt"
	"net/url"
	"strconv"
)

func getFromTo(u *url.URL) (int, int, error) {
	q := u.Query()
	fromStr := q.Get("from")
	toStr := q.Get("to")

	from, to := 0, 0
	var err error

	if from, err = strconv.Atoi(fromStr); err != nil {
		return from, -1, fmt.Errorf("invalid `from` value %s, expected index", fromStr)
	}
	if to, err = strconv.Atoi(toStr); err != nil {
		return from, to, fmt.Errorf("invalid `to` value %s, expected index", toStr)
	}
	if from > to {
		return from, to, fmt.Errorf("`to` should be more or equal to `from`")
	}

	return from, to, nil
}
