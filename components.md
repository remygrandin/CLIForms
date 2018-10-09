# Components

## List of components
## Containers

### Container
#### Desctiption
A simple rectangular container. Inner children render will be clamped to the dimentions of the container.
Inherit from **DisplayObject**
#### Screenshot
None

#### Constructor Parameters

| Name | Type | Default | Description |
|--|--|--|--|
| parent | Container| - | The parent of this container
| width | Int32 | - | The width of the container
| height| Int32 | - | The height of the container

#### Public Properties
| Name | Type | Default | Description |
|--|--|--|--|
| Width | Int32 | 0 | The width of the container
| Height | Int32 | 0 | The height of the container

#### Public Methods

| Name | Return Type | Arguments | Description |
|--|--|--|:--:|:--:|--|
| GetAllChildren | IEnumerable\<DisplayObject\> | None | Return a flatened recursive list of the container childrens. |
| AddChild | void | DisplayObject **child** | Add **child** to the end of the container display list. The **child** **parent** property will be set to the container, automaticly removing it from any other container. If **child** is already part of the container, nothing will happend. |
| RemoveChild | void | DisplayObject **child** | Remove **child** from the container display list. The **child** **parent** property will be set to **null**. If **child** is not already part of the container, nothing will happend. |
| GetSiblings | IEnumerable\<DisplayObject\>| DisplayObject **child** | Return The container display list. |
| Render | ConsoleCharBuffer | DisplayObject **child** | Return the container and it's children. |
| RenderContainer| ConsoleCharBuffer | DisplayObject **child** | Return the container background. |
#### Events
None


---
### Dialog
#### Desctiption
A bordered dialog with a title. Inner children render will be clamped to the dimentions of the container within the border.
Inherit from [Container](#Container)
#### Screenshot
None

#### Constructor Parameters

| Name | Type | Default | Description |
|--|--|--|--|
| parent | Container| - | The parent of this container
| width | Int32 | - | The width of the container
| height| Int32 | - | The height of the container

#### Public Properties
| Name | Type | Default | Description |
|--|--|--|--|
| Width | Int32 | 0 | The width of the container
| Height | Int32 | 0 | The height of the container

#### Public Methods

| Name | Return Type | Arguments | Description |
|--|--|--|:--:|:--:|--|
| GetAllChildren | IEnumerable\<DisplayObject\> | None | Return a flatened recursive list of the container childrens. |
| AddChild | void | DisplayObject **child** | Add **child** to the end of the container display list. The **child** **parent** property will be set to the container, automaticly removing it from any other container. If **child** is already part of the container, nothing will happend. |
| RemoveChild | void | DisplayObject **child** | Remove **child** from the container display list. The **child** **parent** property will be set to **null**. If **child** is not already part of the container, nothing will happend. |
| GetSiblings | IEnumerable\<DisplayObject\>| DisplayObject **child** | Return The container display list. |
| Render | ConsoleCharBuffer | DisplayObject **child** | Return the container and it's children. |
| RenderContainer| ConsoleCharBuffer | DisplayObject **child** | Return the container background. |
#### Events
None
## Drawings
## Forms
## Globals
## Spinners
## Tables
## Texts
