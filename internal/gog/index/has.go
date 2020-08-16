package index

func Has(indexes map[int]Index, id int) bool {
	_, ok := indexes[id]
	return ok
}
