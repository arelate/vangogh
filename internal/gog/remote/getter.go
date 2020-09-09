package remote

type Getter interface {
	Get(id int) ([]byte, error)
}
