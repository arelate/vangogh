module github.com/boggydigital/vangogh

go 1.17

require (
	github.com/arelate/gog_auth v0.1.3-alpha
	github.com/arelate/gog_media v0.1.3-alpha
	github.com/arelate/gog_types v0.1.8-alpha
	github.com/arelate/gog_urls v0.1.9-alpha
	github.com/arelate/vangogh_downloads v0.1.0
	github.com/arelate/vangogh_extracts v0.1.0-alpha
	github.com/arelate/vangogh_images v0.1.1-alpha
	github.com/arelate/vangogh_products v0.1.3-alpha
	github.com/arelate/vangogh_properties v0.1.0-alpha
	github.com/arelate/vangogh_sets v0.1.0
	github.com/arelate/vangogh_urls v0.1.0-alpha
	github.com/arelate/vangogh_values v0.1.2-alpha
	github.com/boggydigital/clo v0.2.2-alpha
	github.com/boggydigital/dolo v0.2.0
	github.com/boggydigital/gost v0.1.0
	github.com/boggydigital/kvas v0.1.9-alphago
	github.com/boggydigital/nod v0.1.4
	github.com/boggydigital/yt_urls v0.1.1
)

require (
	github.com/arelate/gog_auth_urls v0.1.2-alpha // indirect
	github.com/boggydigital/froth v0.1.0-alpha // indirect
	github.com/boggydigital/match_node v0.1.3 // indirect
	golang.org/x/net v0.0.0-20210525063256-abc453219eb5 // indirect
)

replace (
	github.com/arelate/gog_auth => ../gog_auth
	github.com/arelate/gog_media => ../gog_media
	github.com/arelate/gog_types => ../gog_types
	github.com/arelate/gog_urls => ../gog_urls
	github.com/arelate/vangogh_downloads => ../vangogh_downloads
	github.com/arelate/vangogh_extracts => ../vangogh_extracts
	github.com/arelate/vangogh_images => ../vangogh_images
	github.com/arelate/vangogh_products => ../vangogh_products
	github.com/arelate/vangogh_properties => ../vangogh_properties
	github.com/arelate/vangogh_sets => ../vangogh_sets
	github.com/arelate/vangogh_urls => ../vangogh_urls
	github.com/arelate/vangogh_values => ../vangogh_values
	github.com/boggydigital/clo => ../clo
	github.com/boggydigital/dolo => ../dolo
	github.com/boggydigital/froth => ../froth
	github.com/boggydigital/gost => ../gost
	github.com/boggydigital/kvas => ../kvas
	github.com/boggydigital/nod => ../nod
	github.com/boggydigital/yt_urls => ../yt_urls
)
