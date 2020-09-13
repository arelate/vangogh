package local

import (
	"context"
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/remote/schema"
	"go.mongodb.org/mongo-driver/mongo"
)

type Wishlist struct {
	*Dest
}

func NewWishlist(client *mongo.Client, ctx context.Context) *Wishlist {
	return &Wishlist{
		Dest: NewDest(client, ctx, DB, WishlistCol),
	}
}

func (wdest *Wishlist) Set(wjson interface{}) error {
	var wp schema.Product
	_ = json.Unmarshal(wjson.([]byte), &wp)

	return wdest.Dest.Set(wp)
}
