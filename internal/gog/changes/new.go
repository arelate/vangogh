package changes

import (
	"github.com/boggydigital/vangogh/internal/gog/local/schema"
	"time"
)

func New(id int, hash string) *schema.Change {
	return &schema.Change{
		ID:       id,
		Hash:     hash,
		Added:    time.Now().Unix(),
		Modified: time.Now().Unix(),
	}
}
