package origin

type Getter interface {
	Get(id int) ([]byte, error)
}
