package changes

var changes = make(map[string]Change)

type Change struct {
	Hash     string
	Created  int64
	Modified int64
}
