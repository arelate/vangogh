package urls

type MediaType int

const (
	Game MediaType = iota + 1
	Movie
)

func (mt MediaType) String() string {
	switch mt {
	case Game:
		return "game"
	case Movie:
		return "movie"
	default:
		return ""
	}
}
