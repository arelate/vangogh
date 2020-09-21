// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package names

import "github.com/boggydigital/vangogh/internal/strings/aliases"

func Full(name string) string {

	switch name {
	case Products:
		fallthrough
	case aliases.Products:
		return Products
	case AccountProducts:
		fallthrough
	case aliases.AccountProducts:
		return AccountProducts
	case Wishlist:
		fallthrough
	case aliases.Wishlist:
		return Wishlist
	case Details:
		fallthrough
	case aliases.Details:
		return Details
	default:
		return name
	}
}
