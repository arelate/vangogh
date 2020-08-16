package index

import (
	"time"
)

func created(ix Index) time.Time {
	return time.Unix(ix.Created, 0)
}

func Created(indexes map[int]Index, after, before time.Time) *[]int {
	return filter(indexes, after, before, created)
}
