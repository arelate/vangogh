// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package names

func Changes(col string) string {
	switch col {
	case Products:
		fallthrough
	case AccountProducts:
		fallthrough
	case Wishlist:
		fallthrough
	case Details:
		return col + ChangesSuffix
	default:
		return col
	}
}
