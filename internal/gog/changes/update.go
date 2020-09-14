package changes

import (
	"time"
)

func (chg *schema.Change) Update(hash string) *schema.Change {
	if chg != nil {
		chg.Hash = hash
		chg.Modified = time.Now().Unix()
	}
	return chg
}
