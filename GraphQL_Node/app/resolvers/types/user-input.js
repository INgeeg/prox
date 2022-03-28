"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.UserInput = void 0;
const type_graphql_1 = require("type-graphql");
const class_validator_1 = require("class-validator");
let UserInput = class UserInput {
};
__decorate([
    (0, type_graphql_1.Field)(),
    (0, class_validator_1.Length)(1, 255)
], UserInput.prototype, "username", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, class_validator_1.IsEmail)()
], UserInput.prototype, "email", void 0);
__decorate([
    (0, type_graphql_1.Field)(() => type_graphql_1.ID)
], UserInput.prototype, "cart_id", void 0);
UserInput = __decorate([
    (0, type_graphql_1.InputType)()
], UserInput);
exports.UserInput = UserInput;