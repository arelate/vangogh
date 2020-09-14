package schema

type Change struct {
	ID       int    `json:"id" bson:"_id"`
	Hash     string `json:"hash"`
	Added    int64  `json:"added"`
	Modified int64  `json:"modified"`
}
