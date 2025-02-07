package reductions

func Orders(modifiedAfter int64) error {

	//oa := nod.Begin(" %s...", vangogh_integration.GOGOrderDateProperty)
	//defer oa.End()
	//
	//rdx, err := vangogh_integration.NewReduxWriter(vangogh_integration.GOGOrderDateProperty)
	//if err != nil {
	//	return oa.EndWithError(err)
	//}
	//
	//vrOrders, err := vangogh_integration.NewProductReader(vangogh_integration.Orders)
	//if err != nil {
	//	return oa.EndWithError(err)
	//}
	//
	//gogOrderDates := make(map[string][]string, 0)
	//
	//var modifiedOrders []string
	//if modifiedAfter > 0 {
	//	for id := range vrOrders.Since(modifiedAfter, kevlar.Create, kevlar.Update) {
	//		modifiedOrders = append(modifiedOrders, id)
	//	}
	//} else {
	//	for id := range vrOrders.Keys() {
	//		modifiedOrders = append(modifiedOrders, id)
	//	}
	//}
	//
	//for _, orderId := range modifiedOrders {
	//	order, err := vrOrders.Order(orderId)
	//	if err != nil {
	//		return oa.EndWithError(err)
	//	}
	//
	//	// only consider purchase orders, filter out gifts, refunds
	//	if order.Status != gog_integration.OrderStatusPurchase {
	//		continue
	//	}
	//
	//	orderTimestamp, err := strconv.ParseInt(orderId, 10, 64)
	//	if err != nil {
	//		return oa.EndWithError(err)
	//	}
	//
	//	orderDate := time.Unix(orderTimestamp, 0)
	//
	//	for _, orderProduct := range order.Products {
	//		gogOrderDates[orderProduct.Id] = []string{orderDate.Format("2006.01.02 15:04:05")}
	//	}
	//}
	//
	//if err := rdx.BatchReplaceValues(vangogh_integration.GOGOrderDateProperty, gogOrderDates); err != nil {
	//	return oa.EndWithError(err)
	//}
	//
	//oa.EndWithResult("done")

	return nil
}
