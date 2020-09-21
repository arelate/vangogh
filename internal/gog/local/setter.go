// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package local

import (
	"context"
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/internal/strings/aliases"
	"github.com/boggydigital/vangogh/internal/strings/names"
	"go.mongodb.org/mongo-driver/mongo"
)

type Setter interface {
	Set(data interface{}) error
}

func GetSetterByName(name string, mongoClient *mongo.Client, ctx context.Context) (Setter, error) {
	switch name {
	case aliases.Products:
		fallthrough
	case names.Products:
		return NewProducts(mongoClient, ctx), nil
	case aliases.AccountProducts:
		fallthrough
	case names.AccountProducts:
		return NewAccountProducts(mongoClient, ctx), nil
	case aliases.Wishlist:
		fallthrough
	case names.Wishlist:
		return NewWishlist(mongoClient, ctx), nil
	default:
		return nil, errors.New(fmt.Sprintf("unknown source: %s", name))
	}
}
