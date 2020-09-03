package changes

import "time"

func New(id int, hash string) *Change {
	return &Change{
		ID:       id,
		Hash:     hash,
		Added:    time.Now().Unix(),
		Modified: time.Now().Unix(),
	}
}
