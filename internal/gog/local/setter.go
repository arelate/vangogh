package local

type Setter interface {
	Set(data interface{}) error
}
