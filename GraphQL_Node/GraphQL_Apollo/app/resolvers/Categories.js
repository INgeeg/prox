"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.CategoriesResolver = void 0;
const type_graphql_1 = require("type-graphql");
const Categories_1 = require("../entities/Categories");
let CategoriesResolver = class CategoriesResolver {
    returnSingleCategory(id) {
        return __awaiter(this, void 0, void 0, function* () {
            return yield Categories_1.CategoriesModel.findById({ _id: id });
        });
    }
    returnAllCategories() {
        return __awaiter(this, void 0, void 0, function* () {
            return yield Categories_1.CategoriesModel.find();
        });
    }
    createCategory({ name, description }) {
        return __awaiter(this, void 0, void 0, function* () {
            const category = (yield Categories_1.CategoriesModel.create({
                name,
                description,
            })).save();
            return category;
        });
    }
    deleteCategory(id) {
        return __awaiter(this, void 0, void 0, function* () {
            yield Categories_1.CategoriesModel.deleteOne({ id });
            return true;
        });
    }
};
__decorate([
    (0, type_graphql_1.Query)((_returns) => Categories_1.Categories, { nullable: false }),
    __param(0, (0, type_graphql_1.Arg)('id'))
], CategoriesResolver.prototype, "returnSingleCategory", null);
__decorate([
    (0, type_graphql_1.Query)(() => [Categories_1.Categories])
], CategoriesResolver.prototype, "returnAllCategories", null);
__decorate([
    (0, type_graphql_1.Mutation)(() => Categories_1.Categories),
    __param(0, (0, type_graphql_1.Arg)('data'))
], CategoriesResolver.prototype, "createCategory", null);
__decorate([
    (0, type_graphql_1.Mutation)(() => Boolean),
    __param(0, (0, type_graphql_1.Arg)('id'))
], CategoriesResolver.prototype, "deleteCategory", null);
CategoriesResolver = __decorate([
    (0, type_graphql_1.Resolver)()
], CategoriesResolver);
exports.CategoriesResolver = CategoriesResolver;
