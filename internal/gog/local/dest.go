// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package local

import (
	"context"
	"github.com/boggydigital/vangogh/internal/changes"
	"github.com/boggydigital/vangogh/internal/hash"
	"github.com/boggydigital/vangogh/internal/strings/names"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

type Dest struct {
	MongoClient *mongo.Client
	Ctx         context.Context
	DB          string
	Collection  string
}

func NewDest(client *mongo.Client, ctx context.Context, db string, col string) *Dest {
	return &Dest{
		MongoClient: client,
		Ctx:         ctx,
		DB:          db,
		Collection:  col,
	}
}

// Set data into local database and update changed timestamp as needed.
// General flow of setting the data:
// 1. Calculate SHA256 hash of the data
// 2. Find change entry in the Changes collection
// 3. If the change hash is empty (entry not found) - insert new data entry and change entry
// 4. If the change hash matches computed hash - do nothing, there is already up to date data
// 5. If the change hash is not empty and different from the data - replace the data and update change entry
func (dest *Dest) Set(id int, data interface{}) error {

	col := dest.MongoClient.Database(dest.DB).Collection(dest.Collection)
	changesCol := dest.MongoClient.Database(dest.DB).Collection(names.Changes(dest.Collection))

	h, err := hash.Sha256(data)
	if err != nil {
		return err
	}

	var chg changes.Change
	err = changesCol.FindOne(dest.Ctx, bson.M{"_id": id}).Decode(&chg)
	if err != nil {
		switch err {
		case mongo.ErrNoDocuments:
			// No document has been found - silently handle the error
			break
		default:
			return err
		}
	}

	switch chg.Hash {
	case h:
		// Do nothing. Data has not changed
	case "":
		_, err = col.InsertOne(dest.Ctx, data)
		if err != nil {
			return err
		}

		_, err = changesCol.InsertOne(dest.Ctx, changes.New(id, h))
		return err
	default:
		_, err = col.ReplaceOne(dest.Ctx, bson.M{"_id": id}, data)
		if err != nil {
			return err
		}

		_, err = changesCol.ReplaceOne(dest.Ctx, bson.M{"_id": id}, chg.Update(h))
		return err
	}
	return nil
}
