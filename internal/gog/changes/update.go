package changes

import "time"

func (chg *Change) Update(hash string) *Change {
	if chg != nil {
		chg.Hash = hash
		chg.Modified = time.Now().Unix()
	}
	return chg
}
