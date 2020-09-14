package local

import (
	"context"
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/const/names"
	"github.com/boggydigital/vangogh/internal/gog/remote/schema"
	"go.mongodb.org/mongo-driver/mongo"
)

type AccountProducts struct {
	*Dest
}

func NewAccountProducts(client *mongo.Client, ctx context.Context) *AccountProducts {
	return &AccountProducts{
		Dest: NewDest(client, ctx, names.DB, names.AccountProducts),
	}
}

func (apsdest *AccountProducts) Set(apjson interface{}) error {
	var ap schema.AccountProduct
	_ = json.Unmarshal(apjson.([]byte), &ap)

	return apsdest.Dest.Set(ap)
}
