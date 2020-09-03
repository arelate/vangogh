package changes

func Get(colName string, id int) (chg *Change, err error) {

	//changesCol := mongoClient.Database(mongocl.VangoghDB).Collection(colName + changesSuffix)
	//
	//err = changesCol.FindOne(ctx, bson.M{"_id": id}).Decode(&chg)
	//if err != nil {
	//	switch err {
	//	case mongo.ErrNoDocuments:
	//		// No document has been found - silently handle the error
	//		break
	//	default:
	//		return chg, err
	//	}
	//}
	//
	//return chg, err
	return nil, nil
}
