package index

import "time"

func filter(indexes map[int]Index, after, until time.Time, change func(Index) time.Time) *[]int {
	ids := make([]int, 0)
	for id, ix := range indexes {
		t := change(ix)
		if t.After(after) &&
			t.Before(until) {
			ids = append(ids, id)
		}
	}
	return &ids
}
