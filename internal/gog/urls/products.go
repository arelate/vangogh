// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package urls

import (
	media "github.com/boggydigital/vangogh/internal/gog/media"
	"net/url"
)

const newestFirst = "release_desc"

func ProductsPageURL(mt media.Type) *url.URL {
	productsPage := &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   ProductPagesPath,
	}
	q := productsPage.Query()
	q.Add("mediaType", mt.String())
	q.Add("sort", newestFirst)
	//q.Add("page", strconv.Itoa(page))
	productsPage.RawQuery = q.Encode()

	return productsPage
}
