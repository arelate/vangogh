// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type ProductPage struct {
	Page
	Ts       interface{} `json:"ts"`
	Products []Product   `json:"products"`
}
