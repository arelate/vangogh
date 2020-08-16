package index

import (
	"time"
)

func modified(ix Index) time.Time {
	return time.Unix(ix.Modified, 0)
}

func Modified(indexes map[int]Index, after, before time.Time) *[]int {
	return filter(indexes, after, before, modified)
}
