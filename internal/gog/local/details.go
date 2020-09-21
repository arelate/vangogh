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

type Details struct {
	*Dest
}

func NewDetails(client *mongo.Client, ctx context.Context) *Details {
	return &Details{
		Dest: NewDest(client, ctx, names.DB, names.Details),
	}
}

func (dsdest *Details) Set(djson interface{}) error {
	var det schema.Details
	_ = json.Unmarshal(djson.([]byte), &det)

	return dsdest.Dest.Set(det)
}
