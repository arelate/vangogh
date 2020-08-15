package changes

import "time"

func filter(after, until time.Time, change func(Change) time.Time) *[]string {
	fs := make([]string, 0)
	for f, ch := range changes {
		t := change(ch)
		if t.After(after) &&
			t.Before(until) {
			fs = append(fs, f)
		}
	}
	return &fs
}
