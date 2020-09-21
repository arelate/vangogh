// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package local

import (
	"context"
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/remote/schema"
	"github.com/boggydigital/vangogh/internal/strings/names"
	"go.mongodb.org/mongo-driver/mongo"
)

type Wishlist struct {
	*Dest
}

func NewWishlist(client *mongo.Client, ctx context.Context) *Wishlist {
	return &Wishlist{
		Dest: NewDest(client, ctx, names.DB, names.Wishlist),
	}
}

func (wdest *Wishlist) Set(wjson interface{}) error {
	var wp schema.Product
	_ = json.Unmarshal(wjson.([]byte), &wp)

	return wdest.Dest.Set(wp)
}
