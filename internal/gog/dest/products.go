package dest

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/schemas"
	"go.mongodb.org/mongo-driver/mongo"
)

type Products struct {
	*Dest
}

func NewProducts(client *mongo.Client) *Products {
	return &Products{
		Dest: NewDest(client, DB, ProductsCol),
	}
}

func (psdest *Products) Set(pjson interface{}) error {
	var product schema.Product
	_ = json.Unmarshal(pjson.([]byte), &product)

	return psdest.Dest.Set(product)
}
