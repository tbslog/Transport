import { useState, useEffect } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import { useForm } from "react-hook-form";
import CheckboxTree from "react-checkbox-tree";
import "react-checkbox-tree/lib/react-checkbox-tree.css";
import { ToastError } from "../Common/FuncToast";

const CreateRole = (props) => {
  const { getListRole } = props;
  const {
    register,
    reset,
    setValue,
    control,
    clearErrors,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    RoleName: {
      required: "Không được để trống",
      //   maxLength: {
      //     value: 15,
      //     message: "Không được vượt quá 15 ký tự",
      //   },
      minLength: {
        value: 1,
        message: "Không được ít hơn 5 ký tự",
      },
    },
    TrangThai: {
      required: "Không được để trống",
    },
  };

  const [IsLoading, SetIsLoading] = useState(true);
  const [listStatus, setListStatus] = useState([]);
  const [checked, setChecked] = useState([]);
  const [expanded, setExpanded] = useState([]);
  const [nodes, setNodes] = useState([]);

  const onSubmit = async (data) => {
    if (!checked || checked.length < 1) {
      ToastError("Vui Lòng Chọn Quyền");
      return;
    }
    SetIsLoading(true);

    const creaetUser = await postData("User/SetPermissionForRole", {
      RoleName: data.RoleName,
      RoleStatus: data.TrangThai,
      Permission: checked,
    });

    if (creaetUser === 1) {
      getListRole(1);
      handleResetClick();
    }
    SetIsLoading(false);
  };

  useEffect(() => {
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "common",
      ]);
      setListStatus(getStatusList);

      let getTreePermission = await getData("User/GetTreePermission?id=0");
      setNodes(getTreePermission.listTree);

      SetIsLoading(false);
    })();
  }, []);

  const handleResetClick = () => {
    setChecked([]);
    reset();
  };

  return (
    <div className="card card-primary">
      <div className="card-header">
        <h3 className="card-title">Form Thêm Mới Role</h3>
      </div>
      <div>{IsLoading === true && <div>Loading...</div>}</div>

      {IsLoading === false && (
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="card-body">
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="RoleName">Tên Role(*)</label>
                  <input
                    type="text"
                    className="form-control"
                    id="RoleName"
                    {...register(`RoleName`, Validate.RoleName)}
                  />
                  {errors.RoleName && (
                    <span className="text-danger">
                      {errors.RoleName.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="TrangThai">Trạng Thái(*)</label>
                  <select
                    className="form-control"
                    {...register(`TrangThai`, Validate.TrangThai)}
                  >
                    {listStatus &&
                      listStatus.map((val) => {
                        return (
                          <option value={val.statusId} key={val.statusId}>
                            {val.statusContent}
                          </option>
                        );
                      })}
                  </select>
                  {errors.TrangThai && (
                    <span className="text-danger">
                      {errors.TrangThai.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div className="row">
              <div className="form-group">
                <label htmlFor="TrangThai">Phân Quyền(*)</label>
                <CheckboxTree
                  iconsClass="fa5"
                  nodes={nodes}
                  checked={checked}
                  expanded={expanded}
                  showExpandAll={true}
                  showNodeIcon={true}
                  onCheck={(checked) => setChecked(checked)}
                  onExpand={(expanded) => setExpanded(expanded)}
                />
              </div>
            </div>
          </div>
          <div className="card-footer">
            <div>
              <button
                type="button"
                onClick={() => handleResetClick()}
                className="btn btn-warning"
              >
                Làm mới
              </button>
              <button
                type="submit"
                className="btn btn-primary"
                style={{ float: "right" }}
              >
                Thêm mới
              </button>
            </div>
          </div>
        </form>
      )}
    </div>
  );
};

export default CreateRole;
