package reductions

func SteamAppId(since int64) error {

	//saia := nod.Begin(" %s...", vangogh_integration.SteamAppIdProperty)
	//defer saia.Done()
	//
	//rdx, err := vangogh_integration.NewReduxWriter(
	//	vangogh_integration.TitleProperty,
	//	vangogh_integration.SteamAppIdProperty)
	//if err != nil {
	//	return err
	//}
	//
	//vrSteamAppList, err := vangogh_integration.NewProductReader(vangogh_integration.SteamAppList)
	//if err != nil {
	//	return err
	//}
	//
	//vrCatalogProducts, err := vangogh_integration.NewProductReader(vangogh_integration.CatalogProducts)
	//if err != nil {
	//	return err
	//}
	//
	////sal, err := vrSteamAppList.SteamAppList()
	//var sal []string
	//if err != nil {
	//	return err
	//}
	//
	//appMap := GetAppListResponseToMap(sal)
	//gogSteamAppId := make(map[string][]string)
	//
	//for id := range vrCatalogProducts.Since(since, kevlar.Create, kevlar.Update) {
	//
	//	// existing Steam App Id would indicate that we've already matched GOG Id to Steam App Id using
	//	// data sources: HLTB, PCGW, GamesDB and don't need to use potentially lossy mapping by name
	//	if appIds, ok := rdx.GetAllValues(vangogh_integration.SteamAppIdProperty, id); ok && len(appIds) > 0 {
	//		continue
	//	}
	//
	//	title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
	//	if !ok {
	//		continue
	//	}
	//
	//	title = normalizeTitle(title)
	//
	//	if appId, ok := appMap[title]; ok {
	//		gogSteamAppId[id] = []string{strconv.Itoa(int(appId))}
	//	}
	//}
	//
	//if err := rdx.BatchReplaceValues(vangogh_integration.SteamAppIdProperty, gogSteamAppId); err != nil {
	//	return err
	//}

	return nil
}
