// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package local

import (
	"context"
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

func (dest *Dest) Set(data interface{}) error {

	col := dest.MongoClient.Database(dest.DB).Collection(dest.Collection)

	//h, err := hash.Sha256(data)
	//if err != nil {
	//	return err
	//}

	//chg, err := changes.Get(colName, id)
	//if err != nil {
	//	return err
	//}
	//
	//switch chg.Hash {
	//case "":
	_, err := col.InsertOne(dest.Ctx, data)
	if err != nil {
		return err
	}
	//	return changes.Set(colName, changes.New(id, h))
	//case h:
	//	// data unchanged. Do nothing.
	//default:
	//	_, err = col.ReplaceOne(ctx, bson.M{"_id": id}, data)
	//	if err != nil {
	//		return err
	//	}
	//
	//	return changes.Replace(colName, id, chg.Update(h))
	//}
	return nil
}
