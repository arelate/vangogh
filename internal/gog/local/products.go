// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package local

import (
	"context"
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/const/names"
	"github.com/boggydigital/vangogh/internal/gog/remote/schema"
	"go.mongodb.org/mongo-driver/mongo"
)

type Products struct {
	*Dest
}

func NewProducts(client *mongo.Client, ctx context.Context) *Products {
	return &Products{
		Dest: NewDest(client, ctx, names.DB, names.Products),
	}
}

func (psdest *Products) Set(pjson interface{}) error {
	var product schema.Product
	_ = json.Unmarshal(pjson.([]byte), &product)

	return psdest.Dest.Set(product)
}
