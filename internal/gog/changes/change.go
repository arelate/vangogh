package changes

import "time"

type Change struct {
	ID       int    `json:"id" bson:"_id"`
	Hash     string `json:"hash"`
	Added    int64  `json:"added"`
	Modified int64  `json:"modified"`
}

func New(id int, hash string) *Change {
	return &Change{
		ID:       id,
		Hash:     hash,
		Added:    time.Now().Unix(),
		Modified: time.Now().Unix(),
	}
}

func (chg *Change) Update(hash string) *Change {
	if chg != nil {
		chg.Hash = hash
		chg.Modified = time.Now().Unix()
	}
	return chg
}
