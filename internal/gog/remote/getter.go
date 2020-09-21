// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package remote

type Getter interface {
	Get(id int) ([]byte, error)
}
