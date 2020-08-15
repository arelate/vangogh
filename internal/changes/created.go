package changes

import "time"

func created(ch Change) time.Time {
	return time.Unix(ch.Created, 0)
}

func Created(after, before time.Time) *[]string {
	return filter(after, before, created)
}
