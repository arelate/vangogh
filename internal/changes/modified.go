package changes

import "time"

func modified(ch Change) time.Time {
	return time.Unix(ch.Modified, 0)
}

func Modified(after, before time.Time) *[]string {
	return filter(after, before, modified)
}
