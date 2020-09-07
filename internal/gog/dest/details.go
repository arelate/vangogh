package dest

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/origin/schema"
	"go.mongodb.org/mongo-driver/mongo"
)

type Details struct {
	*Dest
}

func NewDetails(client *mongo.Client) *Details {
	return &Details{
		Dest: NewDest(client, DB, DetailsCol),
	}
}

func (dsdest *Details) Set(djson interface{}) error {
	var det schema.Details
	_ = json.Unmarshal(djson.([]byte), &det)

	return dsdest.Dest.Set(det)
}
