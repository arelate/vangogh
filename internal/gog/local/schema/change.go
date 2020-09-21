// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type Change struct {
	ID       int    `json:"id" bson:"_id"`
	Hash     string `json:"hash"`
	Added    int64  `json:"added"`
	Modified int64  `json:"modified"`
}
