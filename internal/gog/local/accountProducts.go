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

type AccountProducts struct {
	*Dest
}

func NewAccountProducts(client *mongo.Client, ctx context.Context) *AccountProducts {
	return &AccountProducts{
		Dest: NewDest(client, ctx, names.DB, names.AccountProducts),
	}
}

func (apsdest *AccountProducts) Set(id int, apjson interface{}) error {
	var ap schema.AccountProduct
	_ = json.Unmarshal(apjson.([]byte), &ap)

	return apsdest.Dest.Set(id, ap)
}
