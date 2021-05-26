package internal

import "sort"

type strInt struct {
	Key string
	Val int
}

type StrIntList struct {
	list []strInt
	desc bool
}

func NewSortList(siMap map[string]int, desc bool) StrIntList {
	siList := StrIntList{
		list: make([]strInt, len(siMap)),
		desc: desc,
	}
	i := 0
	for k, v := range siMap {
		siList.list[i] = strInt{k, v}
		i++
	}
	return siList
}

func (sil StrIntList) Len() int {
	return len(sil.list)
}

func (sil StrIntList) Swap(i, j int) {
	sil.list[i], sil.list[j] = sil.list[j], sil.list[i]
}

func (sil StrIntList) Less(i, j int) bool {
	if sil.desc {
		return sil.list[i].Val > sil.list[j].Val
	} else {
		return sil.list[i].Val < sil.list[j].Val
	}
}

func (sil StrIntList) Sort() {
	sort.Sort(sil)
}

func (sil StrIntList) KeyValues() []strInt {
	return sil.list
}
