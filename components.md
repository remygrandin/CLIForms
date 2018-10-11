
# Components

## List of components

### DisplayObject
#### Desctiption
The base class of all components. Represent a drawable object.
It's an **Abstract** class, an therefore can't be instatiated on it's own
#### Screenshot
None

#### Constructor Parameters

| Name | Type | Default | Description |
|--|--|--|--|
| parent | Container| - | The parent of this displayObject.

#### Public Properties

<!-- tabs:start -->

###### **Class**

| Name | Type | Default | Description |
|--|--|--|--|
| X | Int32 | 0 | The x position relative to the parent center of coordinate. |
| Y | Int32 | 0 | The y position relative to the parent center of coordinate. |
| DisplayX | Int32 | - | **Get Only** ǀ The x position relative to the screen center of coordinate. |
| DisplayY | Int32 | - | **Get Only** ǀ The y position relative to the screen center of coordinate. |
| Parent | Container | null | The parent container. Setting this property will automaticly remove the displayObject from the previous parent display list (if any) and add it to the new parent display list (if any). Can be set to null. |
| Disabled | Boolean | false | If disabled, the object will not trigger any event and will not do anything. Componenets may rely on this property to implement custom disabled behavior, refer to their descriptions of the property for more infos. |
| Dirty | Boolean | false | A flag to know of the object display buffer is dirty and should be redrawn. Setting this property to true will bubble up to the parent chain to the top level and trigger a partial screen redraw. Most if not all modifying action on the displayObject will auto trigger this mechanic. See Engine for more detail. |
| Visible| Boolean | true | The visiblilty of the DisplayObject. If set to false, the object will not be drawn on screen. |

###### **Inherited**

None

<!-- tabs:end -->

#### Public Methods

| Name | Return | Description |
|--|--|--|--|
| Show()| Void | Show the DisplayObject (Short for displayObject.Visible = true). |
| Hide()| Void | Hide the DisplayObject (Short for displayObject.Visible = false). |
| Render() | ConsoleCharBuffer | Abstract method to handle the rendred of the DisplayObject. |

#### Events
None


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
<!-- tabs:start -->
<!-- tab:Class -->
| Name | Type | Default | Description |
|--|--|--|--|
| X | Int32 | 0 | The x position relative to the parent center of coordinate. |
| Y | Int32 | 0 | The y position relative to the parent center of coordinate. |
| DisplayX | Int32 | - | **Get Only** ǀ The x position relative to the screen center of coordinate. |
| DisplayY | Int32 | - | **Get Only** ǀ The y position relative to the screen center of coordinate. |
| Parent | Container | null | The parent container. Setting this property will automaticly remove the displayObject from the previous parent display list (if any) and add it to the new parent display list (if any). Can be set to null. |
| Disabled | Boolean | false | If disabled, the object will not trigger any event and will not do anything. Componenets may rely on this property to implement custom disabled behavior, refer to their descriptions of the property for more infos. |
| Dirty | Boolean | false | A flag to know of the object display buffer is dirty and should be redrawn. Setting this property to true will bubble up to the parent chain to the top level and trigger a partial screen redraw. Most if not all modifying action on the displayObject will auto trigger this mechanic. See Engine for more detail. |
| Visible| Boolean | true | The visiblilty of the DisplayObject. If set to false, the object will not be drawn on screen. |

<!-- tab:Inherited -->
| Name | Type | Default | Description |
|--|--|--|--|
| Width | Int32 | 0 | The width of the container
| Height | Int32 | 0 | The height of the container

<!-- tabs:end -->



#### Public Methods

| Name | Return| Description |
|--|--|--|--|
| GetAllChildren() | IEnumerable\<DisplayObject\> | Return a flatened recursive list of the container childrens. |
| AddChild(DisplayObject **child**) | void |  Add **child** to the end of the container display list. The **child** **parent** property will be set to the container, automaticly removing it from any other container. If **child** is already part of the container, nothing will happend. |
| RemoveChild(DisplayObject **child**) | void | Remove **child** from the container display list. The **child** **parent** property will be set to **null**. If **child** is not already part of the container, nothing will happend. |
| GetSiblings(DisplayObject **child**) | IEnumerable\<DisplayObject\>| Return the container display list. |
| Render() | ConsoleCharBuffer | Return the container and it's children. |
| RenderContainer()| ConsoleCharBuffer | Return the container background. |
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
